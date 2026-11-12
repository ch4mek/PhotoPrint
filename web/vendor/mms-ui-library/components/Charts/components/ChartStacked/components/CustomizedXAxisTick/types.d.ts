import { ChartStackedProjectAdapted } from '../../index.stories';
export interface Values extends ChartStackedProjectAdapted {
    name: Exclude<ChartStackedProjectAdapted['name'], string>;
}
