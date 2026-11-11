import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { FILL_ARTICLE_TEXT } from './constants';
import { EmptyArticleFillerWrapper, PencilIcon } from './styles';

export function EmptyArticleFiller() {
	const theme = useTheme();

	return (
		<EmptyArticleFillerWrapper>
			<PencilIcon color={theme.palette['graphics-primary-2']} />
			<Typography variant="m-400" color={theme.palette['text-black']}>
				{FILL_ARTICLE_TEXT}
			</Typography>
		</EmptyArticleFillerWrapper>
	);
}
