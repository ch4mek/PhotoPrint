import { MenuItem } from '@mms/mms-ui-library/components/Sidebar';
import { JSX, ReactElement, MouseEvent } from 'react';
import { Link } from 'react-router-dom';

import { ProtectedView } from '@/components/ProtectedView';

import { MENU_ITEM_TEST_ID } from './constants';
import { ItemPayload } from './types';

type PropsTypes = {
	label: string;
	icon: ReactElement | JSX.Element;
	selected: boolean;
	onClick: (id: number | null, e: MouseEvent) => void;
	payload: ItemPayload;
	showTooltip: boolean;
	id: number;
	isHidden: boolean;
};

export function RouteGuard({
	icon,
	label,
	selected,
	onClick,
	showTooltip,
	id,
	payload: { roles, slug, claims },
	isHidden,
}: PropsTypes) {
	return (
		<ProtectedView key={slug} slug={slug} roles={roles} claims={claims}>
			<MenuItem
				as={Link}
				to={slug}
				id={id}
				icon={icon}
				selected={selected}
				isShowTooltip={showTooltip}
				label={label}
				onClick={onClick}
				isCompactStyle={isHidden}
				dataTestId={MENU_ITEM_TEST_ID}
			/>
		</ProtectedView>
	);
}
