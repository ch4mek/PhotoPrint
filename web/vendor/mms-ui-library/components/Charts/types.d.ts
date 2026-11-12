export interface TooltipPayload<T extends string | number> {
    x: number;
    y: number;
    width: number;
    height: number;
    tooltipPayload: {
        dataKey: string;
        name: string;
        value: T;
    }[];
}
