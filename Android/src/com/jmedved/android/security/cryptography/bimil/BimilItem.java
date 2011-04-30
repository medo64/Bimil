package com.jmedved.android.security.cryptography.bimil;

import java.io.ByteArrayOutputStream;
import java.util.ArrayList;
import java.util.List;

public class BimilItem {

    private final BimilDocument Document;

    BimilItem(BimilDocument document) {
        this.Document = document;
        this.setTitle("");
        this._records = new ArrayList<BimilRecord>();
    }


    private int _iconIndex;
    public int getIconIndex() {
    	return this._iconIndex;
    }
    public void setIconIndex(int value) {
    	this._iconIndex = value;
    }
    
    private String _title;
    public String getTitle() {
    	return this._title;
    }
    public void setTitle(String value) {
    	this._title = value;
    }

    private List<BimilRecord> _records;
    public List<BimilRecord> getRecords() {
    	return _records;
    }


    public BimilRecord AddRecord(String key, String value, BimilRecordFormat format) {
    	BimilRecord record = new BimilRecord(this.Document, key, value, format);
    	this._records.add(record);
    	return record;
    }

    public BimilRecord AddTextRecord(String key, String value) {
        return AddRecord(key, value, BimilRecordFormat.Text);
    }

    public BimilRecord AddMultilineTextRecord(String key, String value) {
        return AddRecord(key, value, BimilRecordFormat.MultilineText);
    }

    public BimilRecord AddUrlRecord(String key, String value) {
        return AddRecord(key, value, BimilRecordFormat.Url);
    }

    public BimilRecord AddPasswordRecord(String key, String value) {
        return AddRecord(key, value, BimilRecordFormat.Password);
    }


    byte[] GetBytes() {
    	try {
    		ByteArrayOutputStream buffer = new ByteArrayOutputStream();
    		byte[] titleBytes = this.getTitle().getBytes("UTF8");
    		buffer.write(getInt32Bytes(titleBytes.length));
    		buffer.write(titleBytes);
    		buffer.write(getInt32Bytes(this.getIconIndex()));
    		for (BimilRecord record : this.getRecords()) {
    			byte[] keyBytes = record.getKey().getBytes();
    			byte[] valueBytes = record.getValue().getBytes();
    			buffer.write(getInt32Bytes(keyBytes.length));    			
    			buffer.write(getInt32Bytes(valueBytes.length));
    			buffer.write(getInt32Bytes(record.getFormat().getValue()));
    			buffer.write(keyBytes);
    			buffer.write(valueBytes);
	        }
    		return buffer.toByteArray();
    	} catch (Exception ex) {
    		throw new RuntimeException("Cannot save item.", ex);
    	}
    }


    static BimilItem Parse(BimilDocument document, byte[] buffer, int offset, int count) {
        try {
        	if (count < 4) { throw new RuntimeException("Invalid buffer size."); }

        	BimilItem res = new BimilItem(document);

        	int nameLength = getInt32(buffer, offset);
        	res.setTitle(new String(buffer, offset + 4, nameLength, "UTF8"));

        	res.setIconIndex(getInt32(buffer, offset + 4 + nameLength));

        	int currOffset = offset + 4 + nameLength + 4;
        	while (currOffset < (offset + count)) {
        		int keyLength = getInt32(buffer, currOffset);
        		int valueLength = getInt32(buffer, currOffset + 4);
        		BimilRecordFormat type = BimilRecordFormat.valueOf(getInt32(buffer, currOffset + 8));
        		BimilValue key = BimilValue.parse(document, buffer, currOffset + 12, keyLength);
        		BimilValue value = BimilValue.parse(document, buffer, currOffset + 12 + keyLength, valueLength);
        		res.getRecords().add(new BimilRecord(document, key.getText(), value.getText(), type));
        		currOffset += 12 + keyLength + valueLength;
        	}
        	if (currOffset != (offset + count)) { throw new RuntimeException("Invalid buffer content."); }

        	return res;
        } catch (Exception ex) {
            throw new RuntimeException("Cannot parse item.", ex);
        }
    }

    


    private static final byte[] getInt32Bytes(int value) {
    	return new byte[] { (byte)(value >>> 24), (byte)(value >>> 16), (byte)(value >>> 8), (byte)value};
    }
    
    private static final int getInt32(byte[] buffer, int offset) {
        return (buffer[offset+0] << 24) + ((buffer[offset+1] & 0xFF) << 16) + ((buffer[offset+2] & 0xFF) << 8) + (buffer[offset+3] & 0xFF);
    }

}
