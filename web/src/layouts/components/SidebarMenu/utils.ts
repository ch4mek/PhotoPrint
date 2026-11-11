import { useLocation } from 'react-router-dom';

import { Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import {
	HANGFIRE_APP_LINK_TITLE,
	headerMenuLinks,
} from '@/layouts/components/SidebarMenu/constants';

import { HeaderMenuLinks } from './types';

export const checkIfCurrentHrefNotModsenApp =
	(pathname: string) => (menuIcon: HeaderMenuLinks) =>
		!(menuIcon.shouldReplace && pathname.includes(menuIcon.href));

export const getModulesAppLinks = () => {
	const { role, token, claims: userClaims } = useAuth();
	const { pathname } = useLocation();

	return headerMenuLinks
		.filter(
			({ roles, claims }) =>
				(roles as Roles[]).includes(role as Roles) ||
				checkIsHasClaims(claims, userClaims)
		)
		.filter(checkIfCurrentHrefNotModsenApp(pathname))
		.map((link) =>
			link.title !== HANGFIRE_APP_LINK_TITLE
				? link
				: { ...link, href: link.href + token }
		);
};
