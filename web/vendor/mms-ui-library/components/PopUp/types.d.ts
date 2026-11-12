import { CSSProperties, ReactNode, JSX } from 'react';
import type { ButtonProps } from '../Button';
import type { TabsProps } from '../Tabs';
import type { Theme } from '@/types/theme';
export interface DefaultControl extends Pick<ButtonProps, 'view' | 'color'> {
    onClick?: VoidFunction;
    disabled?: boolean;
    tooltipText?: string;
    text?: string;
    customControl?: JSX.Element;
}
export interface ExtraControl extends DefaultControl {
    text: string;
}
export interface Controls {
    positiveControl: DefaultControl;
    negativeControl?: DefaultControl;
    extraControls?: Array<ExtraControl | null>;
}
type PopUpType = 'approve' | 'form' | 'info' | 'delete';
export type HeaderType = 'info' | 'warning';
export interface PopUpProps extends HeaderProps, LayoutProps, Omit<ContainerProps, 'contentStyles'> {
    title: ReactNode;
    type?: PopUpType;
    controls?: Controls;
    tabs?: TabsProps;
    children: ReactNode;
    showCloseButton?: boolean;
    onClose: () => void;
    dataTestId?: string;
    showLoader?: boolean;
    stylesConfig?: PopUpStylesConfig;
    isCloseOnShadow?: boolean;
    onShadowClick?: () => void;
}
interface PopUpStylesConfig {
    content?: CSSProperties;
    button?: CSSProperties;
    loader?: CSSProperties;
    container?: CSSProperties;
    closeButton?: CSSProperties;
}
export interface ContainerProps {
    hasPaddings?: boolean;
    styles?: CSSProperties;
    hasControls?: boolean;
    isTextContent?: boolean;
}
export interface HeaderProps {
    headerType?: HeaderType;
    color?: string;
}
export interface LayoutProps {
    activeShadow?: boolean;
    zIndex?: number;
}
export interface ButtonContainerProps {
    styles?: CSSProperties;
    hasPaddings?: boolean;
}
export interface LoaderWrapperProps {
    styles?: CSSProperties;
}
export interface PopupContainerProps {
    styles?: CSSProperties;
}
export interface GetPopupPaddingsProps {
    hasControls?: boolean;
    hasPaddings?: boolean;
    theme: Theme;
}
export {};
