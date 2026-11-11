import { addOpacityToColor } from '@/helpers/addOpacityToColor';
import {
	BoundaryValuesForColorCalculation,
	MonthCellColors,
} from '@/pages/Payload/contants';
import { ThemeMode } from '@/theme';

const LIGHT_MODE_OPACITY = 1;
const DARK_MODE_OPACIITY = 0.7;

export function getCellBackgroundColor(
	actualWorkload: number,
	theme: ThemeMode
) {
	const opacity =
		theme === ThemeMode.DARK ? DARK_MODE_OPACIITY : LIGHT_MODE_OPACITY;

	if (actualWorkload > BoundaryValuesForColorCalculation.Max) {
		return addOpacityToColor(MonthCellColors.Blue, opacity);
	}

	if (actualWorkload === BoundaryValuesForColorCalculation.Max) {
		return addOpacityToColor(MonthCellColors.Green, opacity);
	}

	if (actualWorkload >= BoundaryValuesForColorCalculation.Medium) {
		return addOpacityToColor(MonthCellColors.Orange, opacity);
	}

	return addOpacityToColor(MonthCellColors.Red, opacity);
}
