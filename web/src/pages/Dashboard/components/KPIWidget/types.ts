import { Resources } from 'i18next';
import { SVGProps } from 'react';

export interface KPIWidgetData {
	monthName: string;
	monthOfYear: number;
	totalActualFte: number | null;
	totalPlannedFte: number | null;
	totalKpi: number | null;
	users: number | null;
	percent: number | null;
	color: string;
}

export type TickProps = SVGProps<SVGTextElement> & {
	x: number;
	y: number;
	index: number;
};

type KpiLegendContentKey = Exclude<
	keyof Resources['dashboard']['kpi'],
	'title'
>;

export type KpiLegendContent = {
	id: string;
	content: KpiLegendContentKey;
	color: string;
};
