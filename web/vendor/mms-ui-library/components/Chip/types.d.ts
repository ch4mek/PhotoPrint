import { CSSProperties, ComponentType, ReactNode } from 'react';
export interface ChipProps extends ChipConfirmPopupProps {
    label?: string;
    withCrossIcon?: boolean;
    disabled?: boolean;
    dataTestId?: string;
    onDelete?: (e?: unknown) => void;
    wrapperStyle?: CSSProperties;
    innerWrapperStyle?: CSSProperties;
    labelStyle?: CSSProperties;
    iconButtonStyle?: CSSProperties;
    disableMessage?: string;
    readonly?: boolean;
}
interface ChipConfirmPopupProps {
    withConfirmPopup?: boolean;
    popupTitle?: string;
    popupContent?: string | ReactNode;
    popupJSXContent?: ComponentType<{
        children: ReactNode;
    }>;
}
export interface ComponentProps {
    isHovered?: boolean;
    disabled?: boolean;
}
export interface ChipContentWrapperProps {
    hasDeleteIcon: boolean;
}
export {};
