import { HSV } from '@/types/colors';
export type InteractiveColorPickerProps = {
    value: HSV;
    onChange: (color: HSV) => void;
    dataTestId?: string;
};
export interface CursorPosition {
    x: number;
    y: number;
}
