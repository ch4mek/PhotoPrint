import { Y_AXIS_MAX_VALUE_PADDING, Y_AXIS_TICKS_COUNT } from '../constants';
import { KPIWidgetData } from '../types';

export function getYAxisTicks(
	data: Pick<KPIWidgetData, 'totalActualFte' | 'users'>[]
): number[] {
	const allValues = data
		.flatMap(({ totalActualFte, users }) => [totalActualFte, users])
		.filter((v): v is number => v !== null);

	const maxValue = Math.max(...allValues, 0);
	const maxWith10Percent = maxValue * Y_AXIS_MAX_VALUE_PADDING;
	const step = maxWith10Percent / Y_AXIS_TICKS_COUNT;

	const value1 = 0;
	const value2 = value1 + step;
	const value3 = value2 + step;
	const value4 = value3 + step;
	const value5 = Math.round(maxWith10Percent);

	return [
		Math.round(value1),
		Math.round(value2),
		Math.round(value3),
		Math.round(value4),
		value5,
	];
}
