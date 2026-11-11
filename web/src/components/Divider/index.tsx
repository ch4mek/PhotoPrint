import { DIVIDER_DEFAULT_COLOR } from './constants';
import { CustomHorizontalRow } from './styles';
import { DividerProps } from './types';

export function Divider({
	color = DIVIDER_DEFAULT_COLOR,
	margin,
}: DividerProps) {
	return <CustomHorizontalRow color={color} margin={margin} />;
}
