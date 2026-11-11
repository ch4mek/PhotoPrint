import { FilterTagWrapper } from '@/components/FilterTag/styles';
import { FilterTagProps, FilterTagType } from '@/components/FilterTag/types';

export function FilterTag({
	id,
	title,
	type = FilterTagType.Main,
	isSelected,
	onClick,
	disabled,
}: FilterTagProps) {
	const handleTagClick = () => {
		if (!disabled) {
			onClick(id);
		}
	};

	return (
		<FilterTagWrapper
			type={type}
			isSelected={isSelected}
			onClick={handleTagClick}
			disabled={disabled}
		>
			{title}
		</FilterTagWrapper>
	);
}
