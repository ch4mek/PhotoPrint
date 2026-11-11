import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { EquipmentTabIcon } from '@mms/mms-ui-library/icons/base/EquipmentTabIcon';
import { PersonTabIcon } from '@mms/mms-ui-library/icons/base/PersonTabIcon';
import { useMemo, useRef, useState } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { useEquipmentContext } from '@/context/EquipmentContext/Equipment';
import { OutsideCloseHandler } from '@/forms/components/FormikWithBackup/types';
import { FormWithSaveExit } from '@/forms/components/FormWithSaveExit';
import { useResize } from '@/hooks/useResize';

import { entityName, EQUIPMENT, StatusEquipment, USER } from './constants';
import {
	EquipmentChangesProvider,
	useEquipmentChangesContext,
} from './context/EquipmentChangesContext';
import { EquipmentUsers } from './EquipmentUsers';
import { MainEquipmentForm } from './MainForm';
import type { EquipmentFormProps, HistoryItem } from './types';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';

function EquipmentFormInner({ onClose, id }: EquipmentFormProps) {
	const { setEquipmentModel } = useEquipmentContext();
	const [activeTab, setActiveTab] = useState(EQUIPMENT);
	const [equipmentId, setEquipmentId] = useState(id || '');
	const [historyItems, setHistoryItems] = useState<HistoryItem[]>([]);
	const [isEquipmentFormValid, setIsEquipmentFormValid] = useState(Boolean(id));
	const [isLoading, setIsLoading] = useState(false);
	const submitFormButtonRef = useRef<HTMLButtonElement>(null);
	const outsideCloseRef = useRef<OutsideCloseHandler>(null);
	const [status, setStatus] = useState<StatusEquipment>(StatusEquipment.NONE);
	const [, windowHeight] = useResize();
	const { hasUnsavedChanges: isFormDirty } = useEquipmentChangesContext();

	const theme = useTheme();

	const submitButtonClick = () => {
		if (submitFormButtonRef?.current) {
			submitFormButtonRef?.current.click();
		}
	};

	const onChangeActiveTab = (newValue: number) => {
		setActiveTab(newValue);
	};

	const tabs = useMemo(
		() => [
			{
				id: 0,
				text: 'Equipment',
				icon: <EquipmentTabIcon />,
			},
			{
				id: 1,
				text: 'User',
				icon: <PersonTabIcon />,
				disabled: !isEquipmentFormValid && activeTab !== USER,
			},
		],
		[isEquipmentFormValid, activeTab]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, activeTab, windowHeight),
		[theme, activeTab, windowHeight]
	);

	return (
		<FormWithSaveExit {...{ isFormDirty, onClose }}>
			{(handleOutsideClick) => (
				<PopUp
					title={
						id
							? getEditingPopUpTitle(entityName)
							: getAddingPopUpTitle(entityName)
					}
					onClose={onClose}
					onShadowClick={handleOutsideClick}
					type="form"
					tabs={{
						data: tabs,
						activeTab,
						onChangeTab: onChangeActiveTab,
					}}
					stylesConfig={popUpStylesConfig}
					controls={
						activeTab === EQUIPMENT
							? {
									negativeControl: { onClick: onClose },
									positiveControl: { onClick: submitButtonClick },
							  }
							: undefined
					}
					showLoader={isLoading}
				>
					{activeTab === EQUIPMENT && (
						<MainEquipmentForm
							{...{
								onClose,
								setEquipmentModel,
								setEquipmentId,
								setActiveTab,
								setStatus,
								submitFormButtonRef,
								outsideCloseRef,
							}}
							id={equipmentId}
							setIsFormValid={setIsEquipmentFormValid}
							onLoad={setIsLoading}
						/>
					)}
					{activeTab === USER && (
						<EquipmentUsers
							{...{
								equipmentId,
								historyItems,
								setHistoryItems,
								isEdit: Boolean(id),
								onLoad: setIsLoading,
								onClose,
								status,
							}}
						/>
					)}
				</PopUp>
			)}
		</FormWithSaveExit>
	);
}

export function EquipmentForm(props: EquipmentFormProps) {
	return (
		<EquipmentChangesProvider>
			<EquipmentFormInner {...props} />
		</EquipmentChangesProvider>
	);
}
