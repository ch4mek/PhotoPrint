import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { NoDataFoundWrapper } from '@/components/NoDataFoundWrapper/styles';
import { NoDataProps } from '@/components/NoDataFoundWrapper/types';

export function NoDataFound({ title, icon, controls }: NoDataProps) {
	const theme = useTheme();

	return (
		<NoDataFoundWrapper>
			{icon}
			<Typography variant="m-400" color={theme.palette['text-black']}>
				{title}
			</Typography>
			{controls}
		</NoDataFoundWrapper>
	);
}
