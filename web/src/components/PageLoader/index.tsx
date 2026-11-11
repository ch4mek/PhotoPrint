import { Spinner } from '@mms/mms-ui-library/components/Spinner';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { Hexagons } from '../Hexagons';

import { PageContainer, Wrapper } from './styles';
import { PageLoaderProps } from './types';

export function PageLoader({ title }: PageLoaderProps) {
	return (
		<PageContainer>
			<Hexagons />
			<Wrapper>
				<Spinner />
				{title && <Typography variant="m-400">{title}</Typography>}
			</Wrapper>
		</PageContainer>
	);
}
