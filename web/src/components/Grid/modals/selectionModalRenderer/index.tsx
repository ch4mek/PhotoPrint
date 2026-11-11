import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { SelectionModalProps } from '@/components/Grid/modals/selectionModalRenderer/types';
import { useCheckboxHandler } from '@/hooks/useCheckboxHandler';

import {
	OptionsSection,
	ControlButtonsWrapper,
	SelectionModalWrapper,
	SelectsSections,
	OptionSection,
} from './styles';

export function SelectionModal({
	onApply,
	onReset,
	selection,
	setSelection,
	applyDisabled,
	resetDisabled,
	options = [],
	allowEmptySelection = true,
	disabledAllSelection = false,
	conteinerMaxHeight,
	extraCheckboxes,
}: SelectionModalProps) {
	const availableOptionsCount = useMemo(
		() => options?.filter((option) => !option.disabled).length,
		[options]
	);
	const { t } = useTranslation('common', { keyPrefix: 'modals' });

	const handleSelectAll = () => {
		if (selection?.length === availableOptionsCount) {
			return setSelection([]);
		}

		setSelection(
			options?.filter((option) => !option.disabled).map(({ id }) => id)
		);
	};

	const handleCheckboxClick = useCheckboxHandler(selection, setSelection);

	const isApplyDisabled = allowEmptySelection
		? applyDisabled
		: !selection.length || applyDisabled;

	return (
		<SelectionModalWrapper maxHeight={conteinerMaxHeight}>
			<SelectsSections>
				<OptionSection
					onClick={disabledAllSelection ? undefined : handleSelectAll}
				>
					<Checkbox
						label={t('selectAll')}
						defaultCheckMark={selection?.length === availableOptionsCount}
						checked={selection?.length > 0}
						disabled={disabledAllSelection}
					/>
				</OptionSection>

				{extraCheckboxes?.map(
					({ label, onClick, isChecked, isDefaultCheckMark }) => (
						<OptionSection key={label} onClick={onClick}>
							<Checkbox
								label={label}
								checked={isChecked}
								defaultCheckMark={isDefaultCheckMark}
							/>
						</OptionSection>
					)
				)}
			</SelectsSections>
			<OptionsSection scroll="vertical" maxHeight="304px" maxWidth="100%">
				{options?.map(({ id, description, disabled }) => (
					<OptionSection
						key={id}
						onClick={disabled ? undefined : handleCheckboxClick(id)}
					>
						<Checkbox
							label={description}
							disabled={disabled}
							textCut
							checked={selection?.includes(id)}
						/>
					</OptionSection>
				))}
			</OptionsSection>
			<ControlButtonsWrapper
				applyDisabled={isApplyDisabled}
				onApply={onApply}
				onReset={onReset}
				resetDisabled={resetDisabled}
			/>
		</SelectionModalWrapper>
	);
}
