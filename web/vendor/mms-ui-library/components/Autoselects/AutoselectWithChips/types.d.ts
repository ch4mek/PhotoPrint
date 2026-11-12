import { Placement } from '@floating-ui/react';
import { ComponentType, CSSProperties, FocusEventHandler, ReactNode } from 'react';
import type { AutoselectTypes, StylesConfig } from '../types';
import { ChipProps } from '@/components/Chip';
import type { InputTypography } from '@/components/Input';
import { CheckIfOptionEqualToValue, GetKey, GetOptionLabel } from '@/types/common';
export interface AutoselectWithChipsStylesConfig extends StylesConfig {
    typography?: InputTypography;
}
export type OnCreateOption<T> = (title: string) => Promise<T | T[] | undefined>;
export type OnChange<T> = (option: T[]) => void;
export type HintTrigger = 'click' | 'hover';
export interface CustomDropdownRenderProps<T> {
    options: T[];
    value: T[];
    search: string;
    onChange: (value: T | T[]) => void;
    onClose: () => void;
}
export interface AutoSelectWithChipsProps<T> {
    value: T[];
    options: T[];
    valueField?: string;
    iconField?: string;
    subValueField?: string;
    groupByField?: string;
    errorText?: string;
    label: string;
    error?: boolean;
    disabled?: boolean;
    size?: AutoselectTypes;
    width?: number;
    maxHeight?: number;
    disableClearable?: boolean;
    chipComponent?: React.ComponentType<ChipProps>;
    limitChips?: number;
    /**
     * Controls whether visible chips should get equal maxWidth when limitChips is active.
     * Enabled by default to preserve previous behavior.
     */
    applyLimitChipMaxWidth?: boolean;
    chipsDisabled?: boolean;
    /** When true, chip is rendered with readOnlyChipComponent (no delete). */
    checkIfChipReadOnly?: (option: T) => boolean;
    labelAndErrorBgColor?: string;
    noOptionsText?: string;
    name?: string;
    onChange: OnChange<T>;
    onFocus?: FocusEventHandler<HTMLInputElement>;
    onBlur?: FocusEventHandler<HTMLInputElement>;
    onSearchInputChange?: (value: string) => void;
    onCreateOption?: OnCreateOption<T>;
    renderCreateOptionLabel?: (input: string) => string;
    createOptionPredicate?: (input: string, options: T[]) => boolean;
    createOptionId?: string;
    getOptionLabel?: GetOptionLabel<T>;
    checkIfChipDisabled?: (option: T) => boolean;
    getKey?: GetKey<T>;
    checkIfOptionEqualToValue?: CheckIfOptionEqualToValue<T>;
    containerStyle?: CSSProperties;
    chipsContainerStyle?: CSSProperties;
    stylesConfig?: AutoselectWithChipsStylesConfig;
    withConfirmPopup?: boolean;
    popupTitle?: string;
    popupContent?: string | ReactNode;
    popupJSXContent?: ComponentType<{
        children: ReactNode;
    }>;
    clearAllPopupTitle?: string;
    clearAllPopupContent?: string | ReactNode;
    onOptionSelected?: (option: T) => void;
    getDisabledMessage?: (option: T) => string | undefined;
    isRequired?: boolean;
    dropTabItemSpacing?: number;
    dataTestId?: string;
    hintTrigger?: HintTrigger;
    popoverPosition?: Placement;
    popoverFallbackPlacements?: Placement[];
    renderDropdown?: (props: CustomDropdownRenderProps<T>) => ReactNode;
}
