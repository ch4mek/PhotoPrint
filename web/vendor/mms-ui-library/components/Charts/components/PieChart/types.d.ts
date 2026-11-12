import { PieSectorDataItem } from 'recharts/types/polar/Pie';
import { ActiveShape } from 'recharts/types/util/types';
export type PieChartSize = 'extraSmall' | 'small' | 'medium' | 'large' | 'extraLarge';
export interface CustomPieChartProps {
    activeIndex?: number;
    onPieEnter?: (index?: number) => void;
    data: {
        name: string | null | undefined;
        value: number;
        color: string;
    }[];
    size?: PieChartSize;
    innerRadius?: string | number;
    outerRadius?: string | number;
    activeShape?: ActiveShape<PieSectorDataItem>;
}
