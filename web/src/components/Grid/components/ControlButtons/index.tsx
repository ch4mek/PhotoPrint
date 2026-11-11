import { useTranslation } from 'react-i18next';

import { ControlButtonsWrapper, ControlButton } from './styles';
import { ControlButtonsProps } from './types';

export function ControlButtons({
	applyDisabled,
	onApply,
	resetDisabled,
	onReset,
	...other
}: ControlButtonsProps) {
	const { t } = useTranslation('common', { keyPrefix: 'controlButtons' });

	return (
		<ControlButtonsWrapper {...other}>
			<ControlButton
				type="reset"
				color="secondary"
				disabled={resetDisabled}
				onClick={onReset}
			>
				{t('reset')}
			</ControlButton>
			<ControlButton type="submit" disabled={applyDisabled} onClick={onApply}>
				{t('apply')}
			</ControlButton>
		</ControlButtonsWrapper>
	);
}
