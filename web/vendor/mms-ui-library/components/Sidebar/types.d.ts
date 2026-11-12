import { CSSProperties, Dispatch, PropsWithChildren, ReactElement, SetStateAction, JSX } from 'react';
export interface SidebarProps extends PropsWithChildren {
    /** Is sidebar render items in a row or a column */
    isCompactView?: boolean;
    isShowMenuTooltips?: boolean;
    /** Grouped menu points */
    menuItems: ItemMenu[][];
    /** Last menu item with bottom fixed styles */
    lastItem?: ReactElement;
    /** Custom render function for rendering custom menu items */
    render?: (menuItem: ItemMenu, isSelected: boolean, isRowStyle: boolean, setIsShowMoreOpen: Dispatch<SetStateAction<boolean>>) => ReactElement;
    /** id of current selected menu item */
    selectedId: number | null;
    /** Function for changing current select menu item id */
    setSelectedId: (id: number | null) => void;
    /** z-index style prop for entire sidebar */
    zIndex?: number;
    /** Style for Sidebar container */
    style?: CSSProperties;
    dataTestId?: string;
}
export interface ItemMenu {
    id: number;
    label: string;
    icon: ReactElement | JSX.Element;
    payload?: unknown;
}
export interface SideMenuContainerProps {
    zIndex: number;
    isCompactView: boolean;
}
