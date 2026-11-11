import { useMemo } from 'react';

import { RenderModalProps, SelectionModal } from '@/components/Grid';
import { techDepartments } from '@/constants/departments';
import { SELECT_TECH_DEPARTMENTS_CHECKBOX_LABEL } from '@/pages/Payload/components/SelectionModalWithDepartments/constants';

export function SelectionModalWithDepartments(
	props: RenderModalProps<string[]>
) {
	const { selection, setSelection, options } = props;

	const handleSelectAllTechDepartments = () => {
		setSelection((prevSelection) => {
			const hasTech = prevSelection.some((id) => techDepartments.includes(id));

			return hasTech
				? prevSelection.filter((id) => !techDepartments.includes(id))
				: [
						...prevSelection,
						...(options ?? [])
							.filter(({ id }) => techDepartments.includes(id))
							.map(({ id }) => id),
				  ];
		});
	};

	const availableDepartmentsOptionsCount = useMemo(
		() =>
			(options ?? []).filter(
				({ id, disabled }) => techDepartments.includes(id) && !disabled
			).length,
		[options]
	);

	const { countOfSelectedDepartments, isTechSelected } = useMemo(() => {
		const selectedTech = selection.filter((id) => techDepartments.includes(id));

		return {
			countOfSelectedDepartments: selectedTech.length,
			isTechSelected: selectedTech.length > 0,
		};
	}, [selection]);

	const extraCheckboxes = useMemo(
		() => [
			{
				label: SELECT_TECH_DEPARTMENTS_CHECKBOX_LABEL,
				onClick: handleSelectAllTechDepartments,
				isChecked: isTechSelected,
				isDefaultCheckMark:
					availableDepartmentsOptionsCount === countOfSelectedDepartments,
			},
		],
		[
			isTechSelected,
			availableDepartmentsOptionsCount,
			countOfSelectedDepartments,
		]
	);

	return <SelectionModal {...props} extraCheckboxes={extraCheckboxes} />;
}
