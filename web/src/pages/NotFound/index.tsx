import { Link } from 'react-router-dom';

import notFound from '@/assets/notFound/notFound.svg';
import { NOT_FOUND_MESSAGE } from '@/constants/errors';
import { Routes } from '@/constants/routes';

import {
	DescriptionText,
	HeaderText,
	MessageText,
	NotFoundPageContainer,
} from './styles';

export function NotFoundPage() {
	return (
		<NotFoundPageContainer>
			<img src={notFound} alt="not found" loading="lazy" />
			<HeaderText variant="m-400" Component="h1">
				Oops!
			</HeaderText>
			<MessageText variant="m-400" Component="h5">
				{NOT_FOUND_MESSAGE}
			</MessageText>
			<DescriptionText variant="l-400" Component="h6" align="center">
				The page you&apos;re looking for might be removed or temporary
				unavailable <Link to={Routes.home}>Go home.</Link>
			</DescriptionText>
		</NotFoundPageContainer>
	);
}
