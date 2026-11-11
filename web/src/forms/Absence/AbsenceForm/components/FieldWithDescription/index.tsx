import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { spaces } from '@mms/mms-ui-library/theme';
import { PropsWithChildren } from 'react';

import { FieldWithDescriptionWrapper } from './styles';
import { FieldWithDescriptionProps } from './types';

export function FieldWithDescription({
	description,
	children,
	gapSize = spaces.s,
}: PropsWithChildren<FieldWithDescriptionProps>) {
	const theme = useTheme();

	return (
		<FieldWithDescriptionWrapper {...{ gapSize }}>
			{children}
			<Typography variant="s-400" color={theme.palette['text-gray']}>
				{description}
			</Typography>
		</FieldWithDescriptionWrapper>
	);
}
