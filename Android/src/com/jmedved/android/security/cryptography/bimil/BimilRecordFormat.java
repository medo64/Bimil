package com.jmedved.android.security.cryptography.bimil;

public enum BimilRecordFormat {
    Text (0),
    MultilineText (1),
    Url (10), 
    Password (20);
    
        
    BimilRecordFormat(int value) {
    	this._value = value;
    }

    private final int _value;
    public final int getValue() {
    	return _value;
    }

    
    public static BimilRecordFormat valueOf(int value) {
    	for (BimilRecordFormat iFormat : BimilRecordFormat.values()) {
    		if (iFormat.getValue() == value) {
    			return iFormat;
    		}
    	}
    	return BimilRecordFormat.Text;
    }

}
