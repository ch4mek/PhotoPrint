import { MouseEvent } from 'react';
import { CursorPosition } from '../types';
export declare const useCanvasHandlers: (onCursorPositionChange: (position: CursorPosition) => void) => {
    handleMouseDown: (event: MouseEvent<HTMLCanvasElement>) => void;
    handleMouseMove: (event: React.MouseEvent<HTMLCanvasElement>) => void;
    handleMouseUp: () => void;
};
