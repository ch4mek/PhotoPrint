import { Button } from '@mms/mms-ui-library/components/Button';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useTranslation } from 'react-i18next';

import type { ButtonAddProps } from './types';

export function ButtonAdd({
	onAdd,
	disabled,
	showTooltip = false,
	tooltipMessage,
	title,
	view = 'filled',
}: ButtonAddProps) {
	const { t } = useTranslation('common', { keyPrefix: 'controlButtons' });
	const btnText = title || t('add');

	return (
		<Tooltip text={tooltipMessage} open={showTooltip}>
			<Button
				isFullWidth={false}
				onClick={onAdd}
				disabled={disabled}
				view={view}
			>
				{btnText}
			</Button>
		</Tooltip>
	);
}
