export type ReviewStarsProps = {
    rating: number;
    onChange?: (rating: number) => void;
    editMode?: boolean;
    score?: boolean;
    count?: number;
    className?: string;
};
export type StarState = 'filled' | 'half' | 'empty';
