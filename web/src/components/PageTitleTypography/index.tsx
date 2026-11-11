import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { PageTitleTypographyProps } from '@/components/PageTitleTypography/types';

export function PageTitleTypography({
	title,
	color,
	stylesConfig,
}: PageTitleTypographyProps) {
	const theme = useTheme();

	return (
		<Typography
			variant="large"
			color={color || theme.palette['text-black']}
			style={stylesConfig}
		>
			{title}
		</Typography>
	);
}
