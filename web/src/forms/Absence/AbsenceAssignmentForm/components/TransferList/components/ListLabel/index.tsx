import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { ListLabelWrapper } from './styles';
import { ListLabelProps } from './types';

export function ListLabel({ label, count }: ListLabelProps) {
	const theme = useTheme();

	return (
		<ListLabelWrapper>
			<Typography variant="s-400" color={theme.palette['text-gray']}>
				{label}:
			</Typography>
			<Typography variant="m-600">{count}</Typography>
		</ListLabelWrapper>
	);
}
