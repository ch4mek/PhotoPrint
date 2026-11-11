import { emptyKPIColorsFormValues } from '../constants';
import { KPIColorsFormValues } from '../types';

import { KPIColorResponse } from '@/queries/settings/kpiColors/types';

export const getKPIFormValuesFromResponse = (
	kpiColorsResponse?: Array<KPIColorResponse>
): KPIColorsFormValues => {
	const mappedKPIColors = kpiColorsResponse
		?.map(({ rightIntervalBorder, ...otherFields }) => ({
			...otherFields,
			rightIntervalBorder: String(rightIntervalBorder),
		}))
		.sort(
			(firstColor, secondColor) =>
				Number(firstColor.rightIntervalBorder) -
				Number(secondColor.rightIntervalBorder)
		);

	return mappedKPIColors
		? {
				minColor: mappedKPIColors[0],
				middleColor: mappedKPIColors[1],
				maxColor: mappedKPIColors[mappedKPIColors.length - 1],
		  }
		: emptyKPIColorsFormValues;
};
