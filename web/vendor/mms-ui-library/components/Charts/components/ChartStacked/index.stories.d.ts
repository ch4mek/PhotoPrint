import { Meta, StoryObj } from '@storybook/react';
import { ChartStacked } from './ChartStacked';
declare const meta: Meta<typeof ChartStacked>;
export default meta;
type Story = StoryObj<typeof meta>;
interface ChartStackedProjectItemResponse {
    projectId: string;
    projectName: string;
    totalTime: number;
    projectColor: string;
    isBillableProject: boolean;
}
interface ChartStackedProjectItemAdapted extends ChartStackedProjectItemResponse {
    hours: number;
    minutes: number;
    hoursForBar: number;
}
export interface ChartStackedProjectAdapted {
    name: {
        date: number;
        weekDay: string;
    } | string;
    projects: ChartStackedProjectItemAdapted[];
    totalTimeInMinutes: number;
}
export declare const MonthlyStats: Story;
export declare const QuarterStats: Story;
export declare const YearStats: Story;
