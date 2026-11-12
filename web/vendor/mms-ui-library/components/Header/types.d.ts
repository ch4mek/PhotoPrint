import { CSSProperties, ReactNode, JSX } from 'react';
export interface HeaderProps {
    /** Logo element */
    logo: string | ReactNode;
    /** Buttons kit on the right side of Header */
    userMenu?: JSX.Element;
    /** Left header Component  */
    navigation?: JSX.Element;
    /** Notification Button Component  */
    notificationButton?: JSX.Element;
    /** Is Fixed Position? */
    isFixed?: boolean;
    /** Zindex for Header */
    zIndex?: number;
    /** Additional content on the right side of Header */
    additionalContent?: ReactNode;
    style?: CSSProperties;
    isLeftMenuSidebarOpen?: boolean;
    dataTestId?: string;
}
export type StyledHeaderProps = {
    zIndex: number;
} & Pick<HeaderProps, 'isFixed'>;
