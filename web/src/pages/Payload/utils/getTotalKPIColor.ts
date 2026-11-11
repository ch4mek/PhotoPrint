import { Theme } from '@emotion/react';

import { KpiColors } from '../types';

import { KPI_COLOR_KEYS } from '@/pages/Settings/components/KPITab/constants';

export function getTotalKPIColor(kpiColor: KpiColors, { palette }: Theme) {
	switch (kpiColor) {
		case KpiColors.Orange:
			return palette[KPI_COLOR_KEYS.middleColor];
		case KpiColors.Green:
			return palette[KPI_COLOR_KEYS.maxColor];
		default:
			return palette[KPI_COLOR_KEYS.minColor];
	}
}
