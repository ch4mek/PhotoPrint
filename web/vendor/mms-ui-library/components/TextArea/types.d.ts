import { ChangeEvent, CSSProperties, HTMLProps, ReactNode, RefAttributes } from 'react';
import { type TextareaAutosizeProps } from 'react-textarea-autosize';
import { SpeakerphoneStylesConfig } from '../Speakerphone';
import { ErrorTextStyles } from '@/components/ErrorText';
import type { InputLabelStyles, InputSizes, InputTypography } from '@/components/Input';
import type { BodyFontSize } from '@/types/emotion';
export interface StylesConfig {
    typography?: InputTypography;
    border?: CSSProperties;
    textArea?: CSSProperties;
    label?: InputLabelStyles;
    errorText?: ErrorTextStyles;
    speakerphoneStylesConfig?: SpeakerphoneStylesConfig;
}
export interface TextAreaProps extends Omit<HTMLProps<HTMLTextAreaElement>, 'size' | 'style' | 'ref'>, TextareaAutosizeProps, RefAttributes<HTMLDivElement> {
    size: keyof InputSizes;
    value: string;
    onChange: (value: ChangeEvent<HTMLTextAreaElement>) => void;
    errorText?: string;
    label: string;
    error?: boolean;
    maxRows?: number;
    name?: string;
    minHeight?: number;
    minRows?: number;
    isRequired?: boolean;
    isHighlightOnFocus?: boolean;
    endAdornment?: ReactNode;
    isScrollVisible?: boolean;
    isErrorIconVisible?: boolean;
    stylesConfig?: StylesConfig;
    dataTestId?: string;
    isAbleVoiceInput?: boolean;
}
export interface InputFieldProps {
    error?: boolean;
    inputTypography: BodyFontSize;
    isHasAdornment: boolean;
    isScrollVisible: boolean;
}
export interface ContainerProps {
    sizeField: keyof InputSizes;
    disabled?: boolean;
}
export interface BordersProps {
    isHasAdornment: boolean;
    error?: boolean;
    isFocused?: boolean;
    disabled?: boolean;
    stylesConfig?: StylesConfig;
}
