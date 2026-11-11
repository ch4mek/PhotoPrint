import { useMemo } from 'react';
import { useSearchParams } from 'react-router-dom';

import { searchParamsConfig } from '../config';

import { PopUpSearchParams } from '@/constants/searchParams';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { personalInfoPopupAccessRules } from '@/helpers/personalInfoPopupAccessRules';

import { PopupAccessState } from './types';

export const usePopUpAccessCheck = () => {
	const [searchParams] = useSearchParams();
	const { role, claims: userClaims, id: userId } = useAuth();

	const popupAccessState = useMemo(
		() =>
			Array.from(searchParams.entries()).reduce<PopupAccessState>(
				(popUpParamsAccess, [key, value]) => {
					if (!(key in searchParamsConfig)) {
						return popUpParamsAccess;
					}

					const popUpSearchParam = key as PopUpSearchParams;
					const { roles, claims, isAuthRequired, requireNotOwnProfile } =
						searchParamsConfig[popUpSearchParam];

					if (!isAuthRequired) {
						popUpParamsAccess.allowed.push([popUpSearchParam, value]);

						return popUpParamsAccess;
					}

					const isAllowedByRoles = roles.includes(role);
					const isAllowedByClaims = checkIsHasClaims(claims, userClaims);

					if (!isAllowedByRoles && !isAllowedByClaims) {
						popUpParamsAccess.denied.push(popUpSearchParam);

						return popUpParamsAccess;
					}

					if (requireNotOwnProfile) {
						if (!userId) {
							return popUpParamsAccess;
						}

						const isAllowedByValue = personalInfoPopupAccessRules.notOwnProfile(
							Number(value),
							userId
						);

						if (isAllowedByValue) {
							popUpParamsAccess.allowed.push([popUpSearchParam, value]);
						} else {
							popUpParamsAccess.denied.push(popUpSearchParam);
							popUpParamsAccess.valueDenied.push(popUpSearchParam);
						}
					} else {
						popUpParamsAccess.allowed.push([popUpSearchParam, value]);
					}

					return popUpParamsAccess;
				},
				{ allowed: [], denied: [], valueDenied: [] }
			),
		[searchParams, role, userClaims, userId]
	);

	return popupAccessState;
};
