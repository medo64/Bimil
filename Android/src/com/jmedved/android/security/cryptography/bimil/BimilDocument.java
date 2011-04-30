package com.jmedved.android.security.cryptography.bimil;

import java.io.ByteArrayOutputStream;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.security.SecureRandom;
import java.util.ArrayList;
import java.util.List;

import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

public class BimilDocument {

	final SecureRandom Rng;
	final Cipher CryptoDec;
	final Cipher CryptoEnc;
    private byte[] PasswordSalt;
    
    
	public BimilDocument() {
		try {
			this.Rng = new SecureRandom();
			
			this.CryptoEnc = Cipher.getInstance("AES/CBC/PKCS5Padding");
			this.CryptoDec = Cipher.getInstance("AES/CBC/PKCS5Padding");

            this._items = new ArrayList<BimilItem>();
        } catch (Exception ex) {
            throw new RuntimeException("Cannot create document.", ex);
        }
	}

	public BimilDocument(String password) {
		this();
		try {
			this.PasswordSalt = new byte[16];
			this.Rng.nextBytes(this.PasswordSalt);

			Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password.getBytes("UTF8"), this.PasswordSalt, 4096);
			SecretKeySpec key = new SecretKeySpec(deriveBytes.getBytes(16), "AES");
			IvParameterSpec iv = new IvParameterSpec(deriveBytes.getBytes(16));
			this.CryptoEnc.init(Cipher.ENCRYPT_MODE, key, iv);
			this.CryptoDec.init(Cipher.DECRYPT_MODE, key, iv);
		} catch (Exception ex) {
			throw new RuntimeException("Cannot parse document.", ex);
		}
	}

	
    public void save(OutputStream stream) {
    	try {
    		stream.write(this.PasswordSalt, 0, this.PasswordSalt.length);

    		ByteArrayOutputStream buffer = new ByteArrayOutputStream();
    		buffer.write(new byte[] { 0x41, 0x31, 0x32, 0x38 });
    		for (BimilItem item : this._items) {
    			byte[] bytes = item.GetBytes();
    			buffer.write(getInt32Bytes(bytes.length));
    			buffer.write(bytes);
    		}
    		buffer.write(new byte[] { 0x41, 0x31, 0x32, 0x38 });

    		byte[] encBuffer = this.CryptoEnc.doFinal(buffer.toByteArray(), 0, buffer.size());

    		stream.write(encBuffer, 0, encBuffer.length);
    	} catch (Exception ex) {
    		throw new RuntimeException("Cannot save document.", ex);
    	}
    }
	
    public void save(String fileName) {
    	OutputStream stream = null;
    	try {
    		stream = new FileOutputStream(fileName);
    		this.save(stream);
    	} catch (Exception ex) {
    		throw new RuntimeException("Cannot save document.", ex);
    	} finally {
        	try {
        		if (stream != null) { stream.close(); }
        	} catch (Exception ex) {
        		throw new RuntimeException("Cannot save document.", ex);
        	}    		
    	}
    }

    public void save(OutputStream stream, String newPassword) {    	
    	BimilDocument newDoc = new BimilDocument(newPassword);
        for(BimilItem item : this.getItems()) {
        	BimilItem newItem = newDoc.addItem(item.getTitle(), item.getIconIndex());
            for (BimilRecord record : item.getRecords()) {
            	newItem.AddRecord(record.getKey().getText(), record.getValue().getText(), record.getFormat());
            }
        }
        newDoc.save(stream);
    }

    public void save(String fileName, String newPassword) {    	
    	OutputStream stream = null;
    	try {
    		stream = new FileOutputStream(fileName);
    		this.save(stream, newPassword);
    	} catch (Exception ex) {
    		throw new RuntimeException("Cannot save document.", ex);
    	} finally {
        	try {
        		if (stream != null) { stream.close(); }
        	} catch (Exception ex) {
        		throw new RuntimeException("Cannot save document.", ex);
        	}    		
    	}
    }

	
    public static BimilDocument open(InputStream stream, String password) throws RuntimeException {
        try {
	        BimilDocument doc = new BimilDocument();
	
	        byte[] salt = new byte[16];
	        stream.read(salt, 0, 16);
	        
	        doc.PasswordSalt = salt;
	
			Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password.getBytes("UTF8"), doc.PasswordSalt, 4096);
			SecretKeySpec key = new SecretKeySpec(deriveBytes.getBytes(16), "AES");
	        IvParameterSpec iv = new IvParameterSpec(deriveBytes.getBytes(16));
	        doc.CryptoEnc.init(Cipher.ENCRYPT_MODE, key, iv);
	        doc.CryptoDec.init(Cipher.DECRYPT_MODE, key, iv);
	
	        byte[] encBuffer = new byte[stream.available()];
	        stream.read(encBuffer, 0, encBuffer.length);
	
	        byte[] decBuffer;
	        decBuffer = doc.CryptoDec.doFinal(encBuffer, 0, encBuffer.length);
	        if ((decBuffer[0] != 0x41) || (decBuffer[1] != 0x31) || (decBuffer[2] != 0x32) || (decBuffer[3] != 0x38)) { throw new RuntimeException("Invalid primary identifier."); }
	        if ((decBuffer[decBuffer.length - 4] != 0x41) || (decBuffer[decBuffer.length - 3] != 0x31) || (decBuffer[decBuffer.length - 2] != 0x32) || (decBuffer[decBuffer.length - 1] != 0x38)) { throw new RuntimeException("Invalid secondary identifier."); }
	
	        int currOffset = 4;
	        while (currOffset < (decBuffer.length - 4)) {
	            int itemLen = getInt32(decBuffer, currOffset);
	            BimilItem item = BimilItem.Parse(doc, decBuffer, currOffset + 4, itemLen);
	            doc._items.add(item);
	        
	            currOffset += 4 + itemLen;
	        }
	
	        return doc;
        } catch (Exception ex) {
            throw new RuntimeException("Cannot open document.", ex);
        }
    }

    public static BimilDocument Open(String fileName, String password) {
    	InputStream stream = null; 
    	try {
    		stream = new FileInputStream(fileName);
    		return open(stream, password);
    	} catch (Exception ex) {
    		throw new RuntimeException("Cannot open document.", ex);
    	} finally {
    		if (stream != null) {
    			try {
    				stream.close(); 
    			} catch (Exception ex) {
    				throw new RuntimeException("Cannot open document.", ex);
    			}
    		}
    	}
    }

    
    private List<BimilItem> _items;
    public List<BimilItem> getItems() {
    	return this._items;
    }


    public BimilItem addItem(String title, int iconIndex) {
        BimilItem item = new BimilItem(this);
        item.setTitle(title);
        item.setIconIndex(iconIndex);
        this.getItems().add(item);
        return item;
    }


    
    
    
    private static final byte[] getInt32Bytes(int value) {
    	return new byte[] { (byte)(value >>> 24), (byte)(value >>> 16), (byte)(value >>> 8), (byte)value};
    }
    
    private static final int getInt32(byte[] buffer, int offset) {
        return (buffer[offset+0] << 24) + ((buffer[offset+1] & 0xFF) << 16) + ((buffer[offset+2] & 0xFF) << 8) + (buffer[offset+3] & 0xFF);
    }

}
