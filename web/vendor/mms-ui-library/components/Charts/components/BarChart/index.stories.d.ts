import { Meta, StoryObj } from '@storybook/react';
import { BarChart } from './BarChart';
declare const meta: Meta<typeof BarChart>;
export default meta;
type Story = StoryObj<typeof meta>;
export interface ChartDataItem {
    name: {
        date: number;
        weekDay: string;
    } | string;
    hours: number;
    minutes: number;
}
export declare const MonthlyStats: Story;
export declare const QuarterStats: Story;
export declare const YearStats: Story;
