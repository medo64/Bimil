package com.jmedved.android.security.cryptography.bimil;


public class BimilValue {

    private final BimilDocument Document;

    BimilValue(BimilDocument document) {
        this.Document = document;
        this.setText("");
    }


    public String getText() {
    	try {
    		if (this._bytes == null) { return null; }
    		byte[] decBuffer;
    		decBuffer = this.Document.CryptoDec.doFinal(this._bytes, 0, this._bytes.length);
    		return new String(decBuffer, 16, decBuffer.length - 16, "UTF8");
    	} catch (Exception ex) {
    		throw new RuntimeException("Cannot get value.", ex);
    	}			
    }

    public void setText(String value) {
    	try {
    		if (value == null) { throw new RuntimeException("Value cannot be null."); }

    		byte[] bufferSalt = new byte[16];
    		this.Document.Rng.nextBytes(bufferSalt);
    		byte[] bufferText = value.getBytes("UTF8");
    		byte[] buffer = new byte[16 + bufferText.length];

    		System.arraycopy(bufferSalt, 0, buffer, 0, 16);
    		System.arraycopy(bufferText, 0, buffer, 16, bufferText.length);

    		this._bytes = this.Document.CryptoEnc.doFinal(buffer, 0, buffer.length);
    	} catch (Exception ex) {
    		throw new RuntimeException("Cannot set value.", ex);
    	}			
    }

    private byte[] _bytes;
    byte[] getBytes() {
    	return _bytes;
    }


    static BimilValue parse(BimilDocument document, byte[] buffer, int offset, int count) {
        byte[] encBuffer = new byte[count];
        System.arraycopy(buffer, offset, encBuffer, 0, count);

        BimilValue res = new BimilValue(document);
        res._bytes = encBuffer;
        return res;
    }

    @Override
    public String toString() {
    	return this.getText();
    }
    
}
