import { Typography } from '@mms/mms-ui-library/components/Typography';

import { KeywordsWrapper } from './styles';

export function KeywordsSection({ keywords }: { keywords: string[] }) {
	return (
		<KeywordsWrapper>
			{keywords.map((keyword) => (
				<Typography key={keyword} variant="m-400" Component="p">
					#{keyword}
				</Typography>
			))}
		</KeywordsWrapper>
	);
}
