import { CursorPosition } from '@/components/InteractiveColorPicker/types';
import { HSV, RGB } from '@/types/colors';
export declare function hexToRgb(hex: string): RGB | null;
export declare const rgbToHex: ({ r, g, b }: RGB) => string;
export declare const rgbToHsv: (colorRgb: RGB) => HSV;
export declare const getCoordinatesFromHSV: (hsv: HSV) => CursorPosition;
export declare function getHSVFromCanvasCoordinates(x: number, y: number, hue: number, canvasWidth: number, canvasHeight: number): {
    h: number;
    s: number;
    v: number;
};
export declare function hsvToRgb(hsv: HSV): RGB;
export declare const convertHexToHsv: (hex: string) => HSV;
