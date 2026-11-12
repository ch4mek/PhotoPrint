export type CardItem = {
    title: string;
    value?: string;
};
export type CardProps = {
    width?: number;
    minWidth?: number;
    headerTitle: string;
    cardItems: CardItem[];
    dataTestId?: string;
};
export type CardWrapperProps = {
    width: number;
    minWidth: number;
};
