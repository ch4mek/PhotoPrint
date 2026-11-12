import type { Theme } from '@/types/theme';
import { CellTextProps } from './types';
export declare const EDIT_CELL_CLASS_NAME = "edit-cell";
export declare const overrideTableStyles: (theme: Theme) => import("@emotion/react").SerializedStyles;
export declare const CellWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & CellTextProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
