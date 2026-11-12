import { DropTabTypography } from '../DropTab';
import { InputProps, InputSizes, InputStylesConfig } from '../Input';
type CurrencyBorderColor = 'graphics-accent' | 'graphics-error' | 'graphics-primary-2' | 'graphics-input-stroke';
export interface CurrencyContainerProps {
    borderColor: CurrencyBorderColor;
    width: number;
}
interface CurrencyInputStylesConfig extends InputStylesConfig {
    currencyDropDownTypography?: DropTabTypography;
}
export interface CurrencyInputProps extends Partial<InputProps> {
    onCurrencyChange?: (currency: string) => void;
    onValueChange: (value: string) => void;
    value: string;
    currency: string;
    currenciesOptions?: string[];
    errorText?: string;
    label: string;
    error?: boolean;
    disabled?: boolean;
    size?: keyof InputSizes;
    disabledCurrencyChange?: boolean;
    hideCurrencyChange?: boolean;
    dropdownLabelText?: string;
    stylesConfig?: CurrencyInputStylesConfig;
    showToolTipIfCut?: boolean;
    includeDecimalPart?: boolean;
    dataTestId?: string;
}
export interface ContainerProps {
    borderColor: CurrencyBorderColor;
    disabled?: boolean;
    size: keyof InputSizes;
}
export {};
