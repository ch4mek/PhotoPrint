import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

import { ActionCellRendererProps } from './types';

export const actionCellRenderer = ({
	icon: Icon,
	onClick: handleClick,
}: ActionCellRendererProps) => (
	<ActionButton onClick={handleClick}>
		<Icon />
	</ActionButton>
);
