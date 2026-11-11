import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { useState } from 'react';

import { useExportEquipment } from '../hooks/useExportEquipment';

import { ExportControl } from '@/components/ExportControl';
import { fileExtensionMapping } from '@/constants/fileExtensions';
import { exportOptions } from '@/pages/Equipment/constants';

import { EquipmentExportButtonWrapper } from './styles';
import { EquipmentExportButtonProps } from './types';

export function EquipmentExportControl({
	filters,
	orderBy,
	order,
	allDepartments,
	allEquipmentLocations,
	allEquipmentTypes,
	allAvailabilityTypes,
	isAddButtonShown,
}: EquipmentExportButtonProps) {
	const [format, setFormat] = useState('');

	useExportEquipment(
		format,
		order,
		orderBy,
		filters,
		setFormat,
		allDepartments,
		allEquipmentLocations,
		allEquipmentTypes,
		allAvailabilityTypes
	);

	const handleDownload = ({ value }: PickerButtonOption) => {
		setFormat(fileExtensionMapping[value as keyof typeof fileExtensionMapping]);
	};

	return (
		<EquipmentExportButtonWrapper isAddButtonShown={isAddButtonShown}>
			<ExportControl options={exportOptions} onChange={handleDownload} />
		</EquipmentExportButtonWrapper>
	);
}
