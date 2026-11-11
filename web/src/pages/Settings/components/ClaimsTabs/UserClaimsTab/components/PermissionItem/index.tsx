import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { memo } from 'react';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { splitValueByCapitalLetters } from '@/pages/Settings/utils';

import { CLAIM_NAMES_EXCLUSIONS } from './constants';
import {
	PermissionTitle,
	PermissionWrapper,
	Permissions,
	ClaimTitle,
	CheckboxWrapper,
} from './styles';
import { PermissionItemProps } from './types';

function PermissionItemInner({
	item,
	permissions,
	setActivePermissions,
}: PermissionItemProps) {
	const { name: permissionName, claims } = item;

	const togglePermission = (name: string) => {
		setActivePermissions((prevPermissions) => {
			if (prevPermissions.includes(name)) {
				return prevPermissions.filter((permission) => permission !== name);
			}

			return [...prevPermissions, name];
		});
	};

	return (
		<PermissionWrapper>
			<PermissionTitle variant="s-600">{permissionName}</PermissionTitle>
			<Permissions>
				{claims?.map(({ name }) => {
					const claimName =
						name in CLAIM_NAMES_EXCLUSIONS
							? CLAIM_NAMES_EXCLUSIONS[name]
							: splitValueByCapitalLetters(name);

					return (
						<CheckboxWrapper key={name}>
							<Checkbox
								label={
									<Tooltip
										text={claimName}
										useIfTextCut
										fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
									>
										<ClaimTitle Component="p" variant="m-400">
											{claimName}
										</ClaimTitle>
									</Tooltip>
								}
								onClick={() => {
									togglePermission(name);
								}}
								checked={permissions.includes(name)}
							/>
						</CheckboxWrapper>
					);
				})}
			</Permissions>
		</PermissionWrapper>
	);
}

export const PermissionItem = memo(PermissionItemInner);
