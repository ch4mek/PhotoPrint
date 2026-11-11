import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { TOTAL_USED_TITLE } from '../../constants';

import { TotalUsedInformation } from './styles';
import { TotalUsedTitleProps } from './types';

export function TotalUsedTitle({ totalCount }: TotalUsedTitleProps) {
	const theme = useTheme();

	return (
		<TotalUsedInformation>
			<Typography variant="s-400" color={theme.palette['text-gray']}>
				{TOTAL_USED_TITLE}
			</Typography>
			<Typography variant="m-600">{totalCount}</Typography>
		</TotalUsedInformation>
	);
}
