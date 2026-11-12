import { TransformedData } from '../../hooks/useStackedTransformData';
import { TooltipPayloadItem } from '../../types';
import { TooltipPayload } from '@/components/Charts/types';
export declare function totalTooltip(payload: TooltipPayloadItem<TransformedData, number>[]): {
    content: string;
    hours: number;
    minutes: number;
};
export declare function projectTooltip(tooltipPayload: TooltipPayload<number>): {
    content: string;
    hours: number;
    minutes: number;
};
