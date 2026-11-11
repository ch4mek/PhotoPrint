import { Typography } from '@mms/mms-ui-library/components/Typography';

import { DEFAULT_VALUE } from '../../config';

import { Column, Value } from './styles';
import { DetailFieldProps } from './types';

export const DetailField = ({
	label,
	value = DEFAULT_VALUE,
}: DetailFieldProps) => (
	<Column>
		<Typography variant="s-400">{label}</Typography>
		<Value variant="m-400">{value}</Value>
	</Column>
);
