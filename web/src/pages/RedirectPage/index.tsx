import { Spinner } from '@mms/mms-ui-library/components/Spinner';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useEffect } from 'react';
import { useParams } from 'react-router-dom';

import { NotFoundPage } from '../NotFound';

import { Hexagons } from '@/components/Hexagons';
import { PageLoader } from '@/components/PageLoader';
import { useGetRedirectInfoById } from '@/queries/useGetRedirectInfoById';

import {
	PageContainer,
	ResourceTypography,
	TypographyWrapper,
	Wrapper,
} from './styles';

export function RedirectPage() {
	const { id } = useParams();

	const { data, error, isFetching } = useGetRedirectInfoById(id);

	useEffect(() => {
		if (data) {
			window.location.href = data.redirectTo;
		}
	}, [data]);

	if (isFetching) {
		return <PageLoader title="Looking for resource..." />;
	}

	if (error || !data) {
		return <NotFoundPage />;
	}

	return (
		<PageContainer>
			<Hexagons />
			<Wrapper>
				<Spinner />
				<TypographyWrapper>
					<Typography variant="l-400">
						Redirecting to
						<br />
						<ResourceTypography variant="l-400">
							{data.redirectTo}
						</ResourceTypography>
					</Typography>
				</TypographyWrapper>
			</Wrapper>
		</PageContainer>
	);
}
