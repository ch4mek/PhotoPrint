import { Input } from '@mms/mms-ui-library/components/Input';
import { ChangeEvent } from 'react';
import { useTranslation } from 'react-i18next';

import { ControlButtons } from '../../components';
import { useAutoFocusInput } from '../../hooks/useAutoFocusInput';
import { RenderModalProps } from '../../renderers';

import { ModalWrapper } from './styles';

export function FreeTextModal({
	onApply,
	onReset,
	selection,
	setSelection,
	applyDisabled,
	resetDisabled,
	isOpen = false,
}: RenderModalProps<string>) {
	const inputRef = useAutoFocusInput(isOpen);
	const { t } = useTranslation('common', { keyPrefix: 'placeholders' });

	const handleChangeSearchValue = (e: ChangeEvent<HTMLInputElement>) => {
		e.stopPropagation();

		setSelection(e.target.value);
	};

	return (
		<ModalWrapper>
			<Input
				size="fullWidth"
				value={selection}
				onChange={handleChangeSearchValue}
				autoComplete="off"
				placeholder={t('search')}
				ref={inputRef}
			/>
			<ControlButtons
				onApply={onApply}
				onReset={onReset}
				applyDisabled={applyDisabled}
				resetDisabled={resetDisabled}
			/>
		</ModalWrapper>
	);
}
