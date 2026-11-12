import { ChartDataItem } from '../../index.stories';
export interface Values extends ChartDataItem {
    name: Exclude<ChartDataItem['name'], string>;
}
