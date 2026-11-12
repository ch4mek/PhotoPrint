export declare const validInputValueRegex: RegExp;
export declare const wholePartRegex: RegExp;
export declare const decimalPartRegex: RegExp;
export declare const commasAndDecimalPartRegex: RegExp;
export declare const isDeleteInputKey: (key: string) => key is "Delete" | "Backspace";
export declare const isCurrencySeparatorChar: (character?: string) => boolean;
export declare const restoreDecimalPartAfterDelete: (inputValue: string) => string;
export declare const mapRawCaretToFormatted: (raw: string, rawCaret: number, formatted: string) => number;
export declare const formatResultCurrencyInput: (wholePart: string, decimalPart?: string) => string;
