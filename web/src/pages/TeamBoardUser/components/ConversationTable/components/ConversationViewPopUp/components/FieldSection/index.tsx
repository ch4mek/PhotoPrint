import { DEFAULT_VALUE } from '../../config';

import { Title, Value, Wrapper } from './styles';
import { FieldSectionProps } from './types';

export const FieldSection = ({
	label,
	value,
	children,
	withDivider = true,
}: FieldSectionProps) => (
	<Wrapper withDivider={withDivider}>
		<Title variant="m-700">{label}</Title>
		{children ?? <Value variant="m-400">{value || DEFAULT_VALUE}</Value>}
	</Wrapper>
);
