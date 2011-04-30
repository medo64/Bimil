package com.jmedved.android.security.cryptography.bimil;

public class BimilRecord {

    BimilRecord(BimilDocument document, String key, String value, BimilRecordFormat format) {
        this._key = new BimilValue(document);
        this._key.setText(key);
        this._value = new BimilValue(document);
        this._value.setText(value);
        this._format = format;
    }

    private BimilValue _key;
    public BimilValue getKey() {
    	return this._key;
    }
    
    private BimilValue _value;
    public BimilValue getValue() {
    	return this._value;
    }

    private BimilRecordFormat _format;
    public BimilRecordFormat getFormat() {
    	return this._format;
    }

    @Override
    public String toString() {
    	return this.getKey().toString();
    }
        
}
