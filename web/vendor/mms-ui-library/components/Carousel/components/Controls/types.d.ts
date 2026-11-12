export interface ControlsProps {
    countOfControls: number;
    activeSlideIndex: number;
    onControlClick: (index: number) => void;
}
